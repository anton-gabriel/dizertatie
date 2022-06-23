import * as THREE from './three/three.module.js';
import { OrbitControls } from './three/OrbitControls.js';
import { OBJLoader } from './three/OBJLoader.js';

let renderer, scene, mesh, line;
const frames = [];
const lines = [];

export function renderScene(host) {
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0x00FFFFFF);

  renderer = new THREE.WebGLRenderer({
    antialias: true,
  });

  const camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 0.01, 20);
  camera.position.set(3, 2, 3);
  scene.userData.camera = camera;

  const controls = new OrbitControls(camera, renderer.domElement);
  controls.maxDistance = 10;
  controls.minDistance = 1;
  controls.update();
  scene.userData.controls = controls;

  const gridHelper = new THREE.GridHelper(10, 50);
  scene.add(gridHelper);

  //var geometry = new THREE.BufferGeometry().setFromPoints([]);
  //var material = new THREE.MeshBasicMaterial({ color: 0x006BCADB, side: THREE.DoubleSide });
  //mesh = new THREE.Mesh(geometry, material);
  //geometry.verticesNeedUpdate = true;
  //scene.add(mesh);

  //const wireframe = new THREE.WireframeGeometry(mesh.geometry);
  //var lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000, });
  //line = new THREE.LineSegments(wireframe, lineMaterial);
  //scene.add(line);

  renderer.setSize(host.clientWidth, window.innerHeight);
  host.appendChild(renderer.domElement);

  animate();
  //Call set size again to solve the scrollbar width issue
  renderer.setSize(host.clientWidth, window.innerHeight);
}

function animate() {
  const camera = scene.userData.camera;
  renderer.render(scene, camera);
  requestAnimationFrame(animate);
}

export function resetCamera() {
  if (scene) {
    scene.userData.controls.reset();
  }
}

export function updateScene(data) {
  var vertices = [];

  for (var i = 0; i < data.points.length; i++) {
    var vertex = data.points[i];
    vertices.push(new THREE.Vector3(vertex[0], vertex[1], vertex[2]));
  }

  //update mesh
  mesh.geometry.setFromPoints(vertices);
  const positionAttribute = mesh.geometry.attributes.position;
  const vertex2 = new THREE.Vector3();
  let vertex3;
  for (let i = 0; i < positionAttribute.count; i++) {
    vertex2.fromBufferAttribute(positionAttribute, i); // read vertex
    vertex3 = data.points[i];
    // do something with vertex
    positionAttribute.setXYZ(i, vertex3[0], vertex3[1], vertex3[2]); // write coordinates
  }
  //update line gemoetry
  line.geometry = new THREE.WireframeGeometry(mesh.geometry);
}

export function updateSceneFromObjectFile(index) {
  const reader = new FileReader();
  const input = document.getElementById('fileUpload');
  const file = input.files[index];

  reader.addEventListener('load', async function (event) {
    const contents = event.target.result;
    const object = new OBJLoader().parse(contents);
    //Check if object is mesh
    object.traverse(function (meshChild) {
      if (meshChild instanceof THREE.Mesh) {
        scene.remove(mesh);
        scene.remove(line);

        meshChild.material = new THREE.MeshBasicMaterial({ color: 0x006BCADB, side: THREE.DoubleSide });
        const wireframe = new THREE.WireframeGeometry(meshChild.geometry);
        var lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000, });
        line = new THREE.LineSegments(wireframe, lineMaterial);
        mesh = object;

        scene.add(line);
        scene.add(object);
      }
    });
  }, false);

  reader.readAsText(file);
}

export function updateSceneFromObject(name, content) {
  const reader = new FileReader();
  //Create File from fileName
  const file = new File([content], name);

  reader.addEventListener('load', async function (event) {
    const contents = event.target.result;
    const object = new OBJLoader().parse(contents);
    //Check if object is mesh
    object.traverse(function (meshChild) {
      if (meshChild instanceof THREE.Mesh) {
        scene.remove(mesh);
        scene.remove(line);

        meshChild.material = new THREE.MeshBasicMaterial({ color: 0x006BCADB, side: THREE.DoubleSide });
        const wireframe = new THREE.WireframeGeometry(meshChild.geometry);
        var lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000, });
        line = new THREE.LineSegments(wireframe, lineMaterial);
        mesh = object;

        scene.add(line);
        scene.add(object);
      }
    });
  }, false);

  reader.readAsText(file);
}

export function loadFrame(name, content) {
  const reader = new FileReader();
  //Create File from fileName
  const file = new File([content], name);

  reader.addEventListener('load', async function (event) {
    const contents = event.target.result;
    const object = new OBJLoader().parse(contents);
    //Check if object is mesh
    object.traverse(function (meshChild) {
      if (meshChild instanceof THREE.Mesh) {
        meshChild.material = new THREE.MeshBasicMaterial({ color: 0x006BCADB, side: THREE.DoubleSide });
        const wireframe = new THREE.WireframeGeometry(meshChild.geometry);
        var lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000, });
        line = new THREE.LineSegments(wireframe, lineMaterial);
        frames.push(object);
        lines.push(line);
      }
    });
  }, false);

  reader.readAsText(file);
}

export function clearFrames() {
  frames.length = 0;
  lines.length = 0;
}

export function updateFrameByIndex(index) {
  frames.forEach(f => scene.remove(f));
  lines.forEach(l => scene.remove(l));

  const object = frames[index];
  const line = lines[index];
  if (index < frames.length) {
    scene.remove(object);
    scene.remove(line);

    scene.add(line);
    scene.add(object);
  }
}

window.addEventListener('resize', function () {
  const host = renderer.domElement.parentElement;
  const camera = scene.userData.camera;

  camera.aspect = host.clientWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(host.clientWidth, window.innerHeight);
}, false);