const ctx = document.getElementById('myChart');

const data = {
    labels: [],
    datasets: [{
        data: [],
        borderColor: 'rgb(75, 192, 192)'
    }]
};

var chart = new Chart(ctx, {
    type: 'line',
    data: data,
    options: {
        tooltips: {
            enabled: false
        },
        scales: {
            y: {
                min: 0,
                max: 1,
                type: 'linear',
            },
        },
        animation: {
            animation: false
        },
        spanGaps: true,
        datasets: {
            line: {
                pointRadius: 0
            }
        },
        plugins: {
            legend: {
                display: false
            },
        }
    }
});

const startTime = Date.now();
function getTime() {
    return Math.round((Date.now() - startTime) / 1000);
}

setInterval(function () {
    var newTime = getTime();
    var newValue = Math.random();

    chart.data.labels.push(newTime);
    chart.data.datasets[0].data.push(newValue);

    if (chart.data.labels.length > 500) {
        chart.data.labels.shift();
    }

    if (chart.data.datasets[0].data.length > 500)
        chart.data.datasets[0].data.shift();
}, 16);

setInterval(function () {
    chart.update();
}, 500);