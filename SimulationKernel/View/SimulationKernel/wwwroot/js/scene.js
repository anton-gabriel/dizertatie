import * as THREE from './three/three.module.js';
import { OrbitControls } from './three/OrbitControls.js';

let renderer, scene, mesh, line;

export function renderScene(host) {
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0x00FFFFFF);

  renderer = new THREE.WebGLRenderer({
    antialias: true,
  });

  const camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 0.01, 20);
  camera.position.set(5, 3, 5);
  scene.userData.camera = camera;

  const controls = new OrbitControls(camera, renderer.domElement);
  controls.maxDistance = 10;
  controls.minDistance = 1;
  controls.update();
  scene.userData.controls = controls;

  const gridHelper = new THREE.GridHelper(10, 50);
  scene.add(gridHelper);

  var geometry = new THREE.BufferGeometry().setFromPoints([]);
  var material = new THREE.MeshBasicMaterial({ color: 0x006BCADB, side: THREE.DoubleSide });
  mesh = new THREE.Mesh(geometry, material);
  geometry.verticesNeedUpdate = true;
  scene.add(mesh);

  const wireframe = new THREE.WireframeGeometry(mesh.geometry);
  var lineMaterial = new THREE.LineBasicMaterial({ color: 0x000000, });
  line = new THREE.LineSegments(wireframe, lineMaterial);
  scene.add(line);

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

export function updateScene(points) {
  var vertices = [];
  
  for (var i = 0; i < points.length; i++) {
    var vertex = points[i];
    vertices.push(new THREE.Vector3(vertex[0], vertex[1], vertex[2]));
  }

  //update mesh
  mesh.geometry.setFromPoints(vertices);
  const positionAttribute = mesh.geometry.attributes.position;
  const vertex2 = new THREE.Vector3();
  let vertex3;
  for ( let i = 0; i < positionAttribute.count; i ++ ) {
    vertex2.fromBufferAttribute( positionAttribute, i ); // read vertex
    vertex3 = points[i];
    // do something with vertex
    positionAttribute.setXYZ(i, vertex3[0], vertex3[1], vertex3[2] ); // write coordinates
  }
  //update line gemoetry
  line.geometry = new THREE.WireframeGeometry(mesh.geometry);
}

window.addEventListener('resize', function () {
  const host = renderer.domElement.parentElement;
  const camera = scene.userData.camera;

  camera.aspect = host.clientWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(host.clientWidth, window.innerHeight);
}, false);