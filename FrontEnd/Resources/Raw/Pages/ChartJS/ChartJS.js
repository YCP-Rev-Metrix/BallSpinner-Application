const ctx = document.getElementById('myChart');

const labels = ['0', '1', '2', '3', '4', '5', '6'];
const data = {
    labels: labels,
    datasets: [{
        label: 'My First Dataset',
        data: [65, 59, 80, 81, 56, 55, 40],
        fill: false,
        borderColor: 'rgb(75, 192, 192)',
        tension: 0.1
    }]
};

var chart = new Chart(ctx, {
    type: 'line',
    data: data,
    options: {
        scales: {
            y: {
                type: 'linear',
                display: true,
                ticks: {
                    beginAtZero: true,
                    max: 100
                }
            }
        },
        animation: {
            duration: 0
        }
    }
});

setInterval(function () {
    var indexToUpdate = Math.round(Math.random() * data.labels.length);
    chart.data.datasets[0].data[indexToUpdate] = Math.random() * 100;

    chart.update();
}, 16);