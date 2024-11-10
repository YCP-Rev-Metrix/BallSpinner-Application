import * as THREE from 'three';
import { OrbitControls } from '/Modules/Three/Addons/OrbitControls.js';

const scene = new THREE.Scene();

const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
const width = 500;
const height = 500;

renderer.setSize(width, height);
renderer.setAnimationLoop(render);
renderer.setClearColor(0xffffff, 0);

const camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000);
const controls = new OrbitControls(camera, renderer.domElement);
document.body.appendChild(renderer.domElement);
controls.enablePan = false;

const ballRadius = 0.10795; //4.25in
const geometry = new THREE.SphereGeometry(ballRadius);
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

controls.minDistance = ballRadius * 3;
controls.maxDistance = ballRadius * 10;
const defaultDistance = controls.minDistance;
camera.position.set(-0.5 * defaultDistance, 0.5 * defaultDistance, -0.5 * defaultDistance); controls.update(); //default position

scene.add(mesh)

//grid
const gridColor = 'rgba(43, 114, 128)';

const gridHelperSmall = new THREE.GridHelper(10, 10, gridColor, gridColor);
scene.add(gridHelperSmall);

const gridHelperMedium = new THREE.GridHelper(90, 9, gridColor, gridColor);
scene.add(gridHelperMedium);

const gridHelperLarge = new THREE.GridHelper(800, 9, gridColor, gridColor);
scene.add(gridHelperLarge);

window.addEventListener('resize', onWindowResize);


//Point at which the arrow starts.
const origin = new THREE.Vector3(0, 0, 0);

// length of the arrow.Default is 1.
const length = 0.2;

//hexadecimal value to define color.
const hex = 0xffff00;

const arrowHelper = new THREE.ArrowHelper(new THREE.Vector3(1, 0, 0), mesh.position, length, hex);

scene.add(arrowHelper);

function render() {
    const dir = new THREE.Vector3(0, 1, 0);  // Initial arrow direction in local space
    dir.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHelper.setDirection(dir);           // Update arrow helper's direction

    // Keep the arrow at the center of the ball
    arrowHelper.position.copy(mesh.position);

    renderer.render(scene, camera);
}

var minX = 0;
var minY = 0;
var minZ = 0;

window.data = function (metric, value, time) {
    if (time > minX && metric === 'RotationX') {
        mesh.rotation.x = value;
        minX = time;
    }
    else if (time > minY && metric === 'RotationY') {
        mesh.rotation.y = value;
        minY = time;
    }
    else if (time > minZ && metric === 'RotationZ') {
        mesh.rotation.z = value;
        minZ = time;
    }
}

function onWindowResize() {

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);

    render();
}

onWindowResize();