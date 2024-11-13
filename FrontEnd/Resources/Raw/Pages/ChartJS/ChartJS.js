const ctx = document.getElementById('myChart');

if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    Chart.defaults.color = 'rgb(255, 255, 255)';
}
else {
    Chart.defaults.color = 'rgb(31, 31, 31)';
}

Chart.defaults.elements.line.borderWidth = 1;

const data = {
    labels: [],
    datasets: [{
        data: [],
        borderColor: 'rgb(255, 0, 0)',
    }, {
        data: [],
        borderColor: 'rgb(0, 255, 0)'
    }, {
        data: [],
        borderColor: 'rgb(0, 0, 255)'
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
                type: 'linear',
            },
            x: {
                ticks: {
                    callback: function (value) {
                        return value.toFixed(1); // Display values with 2 decimal places
                    }
                }
            }
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
        },
        responsive: true,
        maintainAspectRatio: false,
    }
});

const startTime = Date.now();
function getTime() {
    return Math.round((Date.now() - startTime) / 1000);
}

window.data = function (metric, value, time) {
    if (metric.endsWith('X')) {
        chart.data.labels.push(time);
        chart.data.datasets[0].data.push(value);
    } else if (metric.endsWith('Y')) {
        chart.data.datasets[1].data.push(value);
    } else if (metric.endsWith('Z')) {
        chart.data.datasets[2].data.push(value);
    }
    else {
        chart.data.labels.push(time);
        chart.data.datasets[0].data.push(value);
    }

    if (chart.data.labels.length > 300)
        chart.data.labels.shift();

    if (chart.data.datasets[0].data.length > 300)
        chart.data.datasets[0].data.shift();

    if (chart.data.datasets[1].data.length > 300)
        chart.data.datasets[1].data.shift();

    if (chart.data.datasets[2].data.length > 300)
        chart.data.datasets[2].data.shift();
}

setInterval(function () {
    chart.update();
}, 500);