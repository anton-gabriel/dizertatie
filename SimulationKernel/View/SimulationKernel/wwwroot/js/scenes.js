import * as THREE from './three/three.module.js';
import { OrbitControls } from './three/OrbitControls.js';

const scenes = [];
const renderers = [];

export async function renderScenes() {
  var hosts = document.getElementsByClassName("host");
  for (var i = 0; i < hosts.length; i++) {
    var host = hosts.item(i);

    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0x00FFFFFF);

    const renderer = new THREE.WebGLRenderer({
      antialias: true,
      canvas: host.querySelector('#canvas'),
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

    scenes.push(scene);
    renderers.push(renderer);

    renderer.setSize(host.clientWidth, host.clientHeight);
    renderer.render(scene, camera);

    animate(scene, renderer);
  }
}

function animate(scene, renderer) {
  // Foreach scene render
  const controls = scene.userData.controls;
  controls.update();
  renderer.render(scene, scene.userData.camera);
  requestAnimationFrame(() => animate(scene, renderer));
}