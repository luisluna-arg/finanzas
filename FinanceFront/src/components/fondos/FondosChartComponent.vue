<template>
    <div>
        <h2>Fondos Chart</h2>
    </div>
</template>

<script>

import * as echarts from 'echarts';

const buildSeries = (type, data) => { type, data };

const buildChart = function () {
    let categories = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

    let chartDom = document.getElementById('main');
    let myChart = echarts.init(chartDom);
    let option = {
        xAxis: {
            type: 'category',
            data: categories
        },
        yAxis: {
            type: 'value'
        },
        series: [
            buildSeries('line', [150, 230, 224, 218, 135, 147, 260])
        ]
    };

    option && myChart.setOption(option);

}

export default {
    mounted() {
        buildChart();
    }
}

</script>