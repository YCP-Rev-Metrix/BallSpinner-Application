import * as THREE from 'three';
import { OrbitControls } from '/Modules/Three/Addons/OrbitControls.js';

const scene = new THREE.Scene();

let rotationQuaternion = new THREE.Quaternion(); // Accumulated total rotation
let intervalQuaternion = new THREE.Quaternion(); // Incremental rotation per timeStep

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


//arrow at the center of the ball
const arrowHelper = new THREE.ArrowHelper(new THREE.Vector3(1, 0, 0), mesh.position, length, hex);

scene.add(arrowHelper);

// arrow out the side
const arrowHelpe = new THREE.ArrowHelper(new THREE.Vector3(1,0, 0), mesh.position, length, hex);

scene.add(arrowHelpe);

// arrow out the side
const arrowHelp = new THREE.ArrowHelper(new THREE.Vector3(0, 2, 0), mesh.position, length, hex);

scene.add(arrowHelp);

// arrow out the side
const arrowHel = new THREE.ArrowHelper(new THREE.Vector3(0, 2, 0), mesh.position, length, hex);

scene.add(arrowHel);

// arrow out the side
const arrowHe = new THREE.ArrowHelper(new THREE.Vector3(0, 2, 0), mesh.position, length, hex);

scene.add(arrowHe);

// arrow out the side
const arrowH = new THREE.ArrowHelper(new THREE.Vector3(0, 2, 0), mesh.position, length, hex);

let x = 0;
let y = 0;
let z = 0;
let initialRotation = new THREE.Matrix4();

scene.add(arrowH);

function render() {
    mesh.quaternion.copy(rotationQuaternion);

    // Update arrows based on mesh rotation
    [arrowHelper, arrowHelpe, arrowHelp, arrowHel, arrowHe, arrowH].forEach((arrow, index) => {
        const baseDirs = [
            new THREE.Vector3(0, 1, 0),
            new THREE.Vector3(1, 0, 0),
            new THREE.Vector3(0, 0, 1),
            new THREE.Vector3(-1, 0, 0),
            new THREE.Vector3(0, 0, -1),
            new THREE.Vector3(0, -1, 0)
        ];
        let dir = baseDirs[index].clone().applyQuaternion(mesh.quaternion);
        arrow.setDirection(dir);
        arrow.position.copy(mesh.position);
    });

    renderer.render(scene, camera);
}

var minX = 0;
var minY = 0;
var minZ = 0;

window.data = function (metric, value, time) {
    console.log(metric + " " + value + " " + time);
    // If the current metric is for the real motor x speed, update motor x RPM for the simulation
    if (metric === 'MotorXFeedback') {
        x = value;
    }
    /*
    Old code. Not needed right now
    if (time > minX && metric === 'MotorXFeedback') {
        x = value;

        initialRotation = new THREE.Matrix4();
        // Convert RPM value to radians
        var RadianX = (x / 60) * (time - minx)
        let euler = new THREE.Euler(
            x,
            y,
            z,
            'XYZ'
        );

        initialRotation.makeRotationFromEuler(euler);

        minX = time;
    }
    */
}

let IntervalID = 0;
window.StartBallRotation = function (timeStep, StartInterval) {
    if (StartInterval) {
        IntervalID = window.setInterval(function () {
            console.log("Interval");

            // Calculate how much to rotate in this time step (radians per timeStep)
            let radiansX = ((x / 60) * timeStep) * 2 * Math.PI;

            // Build incremental quaternion for this rotation step
            let euler = new THREE.Euler(radiansX, 0, 0, 'XYZ');
            intervalQuaternion.setFromEuler(euler);

            // Apply incremental rotation to the accumulated rotation
            rotationQuaternion.multiply(intervalQuaternion);
        }, timeStep * 1000);
    } else {
        window.clearInterval(IntervalID);
    }
}

function onWindowResize() {

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);

    render();
}

onWindowResize();