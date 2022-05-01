import * as THREE from './three/three.module.js';
import { OrbitControls } from './three/OrbitControls.js';


let renderer, scene, mesh, line;
let incrementer = 0;

//export class SceneRenderer {
//  colorInit(host) {

//  }

//  colorHost(host, color) {
//    scene = new THREE.Scene();
//    scene.background = new THREE.Color(0x00FFFFFF);

//    renderer = new THREE.WebGLRenderer({
//      antialias: true,
//    });

//    const camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 0.01, 20);
//    camera.position.set(3, 2, 3);
//    scene.userData.camera = camera;

//    const controls = new OrbitControls(camera, renderer.domElement);
//    controls.maxDistance = 10;
//    controls.minDistance = 1;
//    controls.update();
//    scene.userData.controls = controls;

//    const gridHelper = new THREE.GridHelper(10, 50);
//    scene.add(gridHelper);

//    renderer.setSize(host.clientWidth, host.clientHeight);
//    host.appendChild(renderer.domElement);

//    renderer.render(scene, camera);

//    animate(scene, renderer);
//  }

//  animate() {
//    const camera = scene.userData.camera;
//    renderer.render(scene, camera);
//    requestAnimationFrame(animate);
//  }
//}

export function renderInit(host) {
  incrementer++;
}

export function render(host, color) {
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

  renderer.setSize(host.clientWidth, host.clientHeight);
  host.appendChild(renderer.domElement);

  renderer.render(scene, camera);

  animate(scene, renderer);
}

export function animate() {
  const camera = scene.userData.camera;
  renderer.render(scene, camera);
  requestAnimationFrame(animate);
}