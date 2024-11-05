import * as THREE from 'three';
import { OrbitControls } from '/Modules/Three/Addons/OrbitControls.js';
//import { GLTFLoader } from '/Modules/Three/Addons/GLTFLoader.js'; // Import GLTFLoader


// Scene setup
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x008000);

const width = 500;
const height = 500;

// Renderer setup
const renderer = new THREE.WebGLRenderer();
renderer.setSize(width, height);
document.body.appendChild(renderer.domElement);

// Camera setup
const camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
camera.position.z = 5;

// Orbit Controls
const controls = new OrbitControls(camera, renderer.domElement);

// Load Texture
const textureLoader = new THREE.TextureLoader();
const texture = textureLoader.load('path/to/your/texture.jpg'); // Replace with the path to your texture file


// Sphere geometry and material with polygon offset to avoid z-fighting
const geometry = new THREE.SphereGeometry(1);
const material = new THREE.MeshBasicMaterial({
    color: 0x800000,
    polygonOffset: true,
    polygonOffsetFactor: 1,
    polygonOffsetUnits: 1
});
const mesh = new THREE.Mesh(geometry, material);

// Adding a new box object
const boxGeometry = new THREE.BoxGeometry(4, 0.4, 10); // Box size
const boxMaterial = new THREE.MeshBasicMaterial({ color: 0xFFFFFF });
const boxMesh = new THREE.Mesh(boxGeometry, boxMaterial);
boxMesh.position.set(0, -1, -5); // Position the box in front of the sphere

// Add box to the scene
scene.add(boxMesh);

// Load a 3D model using GLTFLoader
//const loader = new GLTFLoader();
//loader.load(
//    '/path/to/your/model.glb',   // Path to the 3D model
//    (gltf) => {
//        const model = gltf.scene;
//        model.scale.set(1, 1, 1); // Scale model as needed
//        model.position.set(0, 0, 0); // Position model as needed

//        scene.add(model); // Add model to the scene
//    },
//    (xhr) => {
//        console.log((xhr.loaded / xhr.total * 100) + '% loaded'); // Loading progress
//    },
//    (error) => {
//        console.error('An error happened:', error); // Error callback
//    }
//);

// Wireframe overlay
const geo = new THREE.EdgesGeometry(mesh.geometry);
const mat = new THREE.LineBasicMaterial({ color: 0xff0000 });
const wireframe = new THREE.LineSegments(geo, mat);
mesh.add(wireframe);

// Add sphere with wireframe to scene
scene.add(mesh);

// Animation function with forward movement
let lastTime = 0;
const forwardSpeed = 0.001; // Adjust this speed as needed

function animate(time) {
    const deltaTime = time - lastTime;
    lastTime = time;

    // Rotate the sphere
    mesh.rotation.x += deltaTime * -0.01; // Adjust rotation speed as needed

    // Move the sphere forward along the z-axis
    mesh.position.z -= forwardSpeed * deltaTime;

    renderer.render(scene, camera);
}

// Start the animation loop
renderer.setAnimationLoop(animate);
