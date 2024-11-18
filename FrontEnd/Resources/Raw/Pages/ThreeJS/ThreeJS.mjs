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
const initialRotation = new THREE.Matrix4();

scene.add(arrowH);

function render() {
    //arrow at the center of the ball
    const dir = new THREE.Vector3(0, 1, 0);  // Initial arrow direction in local space
    dir.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHelper.setDirection(dir);           // Update arrow helper's direction
    arrowHelper.position.copy(mesh.position);

    // arrow out the side 
    const diy = new THREE.Vector3(1, 0, 0);  // Initial arrow direction in local space
    diy.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHelpe.setDirection(diy);           // Update arrow helper's direction
    arrowHelpe.position.copy(mesh.position);

    // arrow out the side 
    const die = new THREE.Vector3(0, 0, 1);  // Initial arrow direction in local space
    die.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHelp.setDirection(die);           // Update arrow helper's direction
    arrowHelp.position.copy(mesh.position);

    // arrow out the side 
    const dia = new THREE.Vector3(-1, 0, 0);  // Initial arrow direction in local space
    dia.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHel.setDirection(dia);           // Update arrow helper's direction
    arrowHel.position.copy(mesh.position);

    // arrow out the side 
    const dis = new THREE.Vector3(0, 0, -1);  // Initial arrow direction in local space
    dis.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowHe.setDirection(dis);           // Update arrow helper's direction
    arrowHe.position.copy(mesh.position);

    // arrow out the side 
    const dit = new THREE.Vector3(0, -1, 0);  // Initial arrow direction in local space
    dit.applyQuaternion(mesh.quaternion);    // Apply the ball's rotation quaternion
    arrowH.setDirection(dit);           // Update arrow helper's direction
    arrowH.position.copy(mesh.position);



    renderer.render(scene, camera);
}

var minX = 0;
var minY = 0;
var minZ = 0;

window.data = function (metric, value, time) {
    console.log(metric + " " + value + " " + time);

    if (time > minX && metric === 'RotationX') {
        x = value;
        minX = time;
    }
    else if (time > minY && metric === 'RotationY') {
        y = value;
        minY = time;
    }
    else if (time > minZ && metric === 'RotationZ') {
        z = value;

        if (minZ == 0) {
            initialRotation = new THREE.Matrix4();

            let euler = new THREE.Euler(
                x,
                y,
                z,
                'XYZ'
            );

            initialRotation.makeRotationFromEuler(euler);
        }

        minZ = time;
    }

    let newEuler = new THREE.Euler(THREE.MathUtils.degToRad(x), THREE.MathUtils.degToRad(y), THREE.MathUtils.degToRad(z), 'XYZ');
    let newMatrix = new THREE.Matrix4();
    newMatrix.makeRotationFromEuler(newEuler);

    let combinedMatrix = new THREE.Matrix4();
    combinedMatrix.multiply(newMatrix).multiply(initialRotation);

    let quaternionB = new THREE.Quaternion().setFromRotationMatrix(combinedMatrix);

    // Step 2: Interpolate the quaternions using slerp (spherical linear interpolation)
    let alpha = 0.5;  // Interpolation factor (0.0 is matrixA, 1.0 is matrixB)
    let interpolatedQuaternion = new THREE.Quaternion().slerp(quaternionA, quaternionB, alpha);

    // Step 3: Convert the interpolated quaternion back to a rotation matrix
    let interpolatedMatrix = new THREE.Matrix4().makeRotationFromQuaternion(interpolatedQuaternion);

    mesh.rotation.setFromRotationMatrix(combinedMatrix);
}

function onWindowResize() {

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);

    render();
}

onWindowResize();