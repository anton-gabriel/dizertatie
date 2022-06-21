import pickle
from absl import app
from absl import logging
import numpy as np
import glob
import os
import tensorflow.compat.v1 as tf
import cloth_eval
import cloth_model
import core_model

def process_obj_file(obj_file):
    """ 
    Read .obj file and load the data in vectors.
    """
    with open(obj_file, 'r') as f:
        lines = f.readlines()
    vertices = []
    faces = []
    for line in lines:
        if line.startswith('v '):
            vertices.append(list(map(np.float32, line.split()[1:])))
        elif line.startswith('f '):
            if (line.find('/') == -1):
              faces.append(list(map(int, line.split()[1:])))
            else:
              # Split each element by '/' and get only the first element (ignore texture)
              faces.append([int(x.split('/')[0]) for x in line.split()[1:]])

    return np.array(vertices), np.array(faces)
    

def process_obj_data(data_directory: str):
  #"cells"      (1, nr_of_triangles, 3)         int      "f"
  #"mesh_pos"   (1, nr_points, 2)               float    "v" - (x,y) -> pentru primul frame (z = 0)
  #"node_type"  (nr_obj, nr_points, 1)          int      "0"
  #"world_pos"  (nr_obj, nr_points, 3)          float    "v"
  obj_files = glob.glob(f"{data_directory}/*.obj")

  print(f"Found {len(obj_files)} .obj files")
  first_frame = obj_files[0]
  world_pos, cells = process_obj_file(first_frame)
  cells = cells - 1
  mesh_pos = world_pos[:, :2]
  node_t = np.zeros((world_pos.shape[0], 1), dtype=np.int32)
  "Set node_type to 3 for prindere"
  node_t[np.all(world_pos == 0, axis=1)] = 3
  node_t[np.all(world_pos == [0.0, 2.0, 0.0], axis=1)] = 3
  
  world_pos = np.expand_dims(world_pos, axis=0)
  node_t = np.expand_dims(node_t, axis=0)

  "Iterate over the remaining .obj files and add the remaining word positions and node_type"
  for obj_file in obj_files[1:]:
    world_pos_, _ = process_obj_file(obj_file)
    node_t_ = np.zeros((world_pos_.shape[0], 1), dtype=np.int32)
    node_t_[np.all(world_pos_ == 0, axis=1)] = 3
    node_t_[np.all(world_pos_ == [0.0, 2.0, 0.0], axis=1)] = 3
 
    world_pos_ = np.expand_dims(world_pos_, axis=0)
    node_t_ = np.expand_dims(node_t_, axis=0)

    world_pos = np.concatenate((world_pos, world_pos_), axis=0)
    node_t = np.concatenate((node_t, node_t_), axis=0)

  number_of_frames = len(obj_files)

  mesh_pos = np.expand_dims(mesh_pos, axis=0)
  cells = np.expand_dims(cells, axis=0)
  mesh_pos = np.repeat(mesh_pos, number_of_frames, axis=0)
  cells = np.repeat(cells, number_of_frames, axis=0)

  return {
    "mesh_pos": mesh_pos,
    "world_pos": world_pos,
    "cells": cells,
    "node_type": node_t
  }

def add_targets(data, frames_to_simulate: int):
  """Creates two new keys to the processed .obj dictionary
  'target|mesh_pos' and 'target|world_pos' and adds the targets to the data."""

  if (data['cells'].shape[0] == 2):
    data['prev|world_pos'] = data['world_pos'][:1]
    data['target|world_pos'] = data['world_pos'][1:] # asta nu se foloseste
    data['node_type'] = data['node_type'][1:]
    data['mesh_pos'] = data['mesh_pos'][1:]
    data['cells'] = data['cells'][1:]
    data['world_pos'] = data['world_pos'][1:]
  else:
    data['prev|world_pos'] = data['world_pos'][:-2]
    data['target|world_pos'] = data['world_pos'][2:]
    data['node_type'] = data['node_type'][1:-1]
    data['mesh_pos'] = data['mesh_pos'][1:-1]
    data['cells'] = data['cells'][1:-1]
    data['world_pos'] = data['world_pos'][1:-1]

  return data


def simulate(model, obj_path, num_frames, num_rollouts, rollout_path, checkpoint_dir):
  """Run a model rollout trajectory."""
  ds = process_obj_data(obj_path)
  ds = add_targets(ds, num_frames)

  print(ds['world_pos'].shape)
  print(ds['mesh_pos'].shape)
  print(ds['cells'].shape)
  print(ds['node_type'].shape)
  print(ds['prev|world_pos'].shape)
  print(ds['target|world_pos'].shape)
  
  data_tensor = {k: tf.convert_to_tensor(v) for k, v in ds.items()}
  dataset = tf.data.Dataset.from_tensors(data_tensor)

  inputs = tf.data.make_one_shot_iterator(dataset).get_next()
  scalar_op, traj_ops = cloth_eval.evaluate(model, inputs)
  tf.train.create_global_step()

  with tf.train.MonitoredTrainingSession(
      checkpoint_dir=checkpoint_dir,
      save_checkpoint_secs=None,
      save_checkpoint_steps=None) as sess:
    trajectories = []
    scalars = []
    print(range(num_rollouts))
    for traj_idx in range(num_rollouts):
      logging.info('Rollout trajectory %d', traj_idx)
      scalar_data, traj_data = sess.run([scalar_op, traj_ops])
      trajectories.append(traj_data)
      scalars.append(scalar_data)
    for key in scalars[0]:
      logging.info('%s: %g', key, np.mean([x[key] for x in scalars]))
    with open(rollout_path, 'wb') as fp:
      pickle.dump(trajectories, fp)


def main(argv):
  del argv
  tf.enable_resource_variables()
  tf.disable_eager_execution()
  learned_model = core_model.EncodeProcessDecode(output_size=3, latent_size=128, num_layers=2, message_passing_steps=15)
  model = cloth_model.Model(learned_model)
  #evaluator(model)
  simulate(
    model,
    'D:\\Master\\git_repo\\Simulator\\Data\\out_blender', #input files path
    #'D:\\Master\\git_repo\\Simulator\\Data\\out_flag_twice_sub',
    40,#obj out frames
    1,#rollout number
    'temp/out_flag_blender_rework.pkl',#output file
    'temp/checkpoint'#path dir to trained model
  )

if __name__ == '__main__':
  app.run(main)


def write_obj(filename, verts, faces):
    with open(filename, 'w') as f:
        for v in verts:
            f.write('v %f %f %f\n' % (v[0], v[1], v[2]))

        for face in faces:
            f.write('f %d %d %d\n' % (face[0] + 1, face[1] + 1, face[2] + 1))

# Foreach rollout, create a new folder and save the .obj files in it
def write_rollout_data(full_dir_path, rollout_data):
    for i in range(len(rollout_data)):
        # Create a new folder
        # Save the .obj files
        # gt_pos
        # pred_pos
        for j in range(len(rollout_data[i]["gt_pos"])):
            write_obj(full_dir_path + "/" + str(j) + ".obj", rollout_data[i]["pred_pos"][j], rollout_data[i]["faces"][j])

    return full_dir_path

def write_rollout_pickle(full_dir_path, filename):
    with open(filename, 'rb') as fp:
        rollout_data = pickle.load(fp)
        return write_rollout_data(full_dir_path, rollout_data)