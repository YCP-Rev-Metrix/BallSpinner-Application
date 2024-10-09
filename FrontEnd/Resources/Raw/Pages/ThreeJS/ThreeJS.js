import * as THREE from 'three';
import { OrbitControls } from '/Modules/Three/Addons/OrbitControls.js';
const scene = new THREE.Scene();
scene.background = new THREE.Color(0xffffff);

const renderer = new THREE.WebGLRenderer();
const width = 500;
const height = 500;

renderer.setSize(width, height);
renderer.setAnimationLoop(animate);

const camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
const controls = new OrbitControls(camera, renderer.domElement);
document.body.appendChild(renderer.domElement);

const geometry = new THREE.SphereGeometry(1);
var material = new THREE.MeshBasicMaterial({
    color: 0x800000, polygonOffset: true,
    polygonOffsetFactor: 1, // positive value pushes polygon further away
    polygonOffsetUnits: 1
});
var mesh = new THREE.Mesh(geometry, material);
var geo = new THREE.EdgesGeometry(mesh.geometry);
var mat = new THREE.LineBasicMaterial({ color: 0xff0000 });
var wireframe = new THREE.LineSegments(geo, mat);
mesh.add(wireframe);

scene.add(mesh)
camera.position.z = 5;

function animate() {

    renderer.render(scene, camera);
    mesh.rotation.x += 3.1415926 / 360;
}