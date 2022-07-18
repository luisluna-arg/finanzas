<template>
    <div>
        <h1>Fondos</h1>    
        <FondosGridComponent></FondosGridComponent>
        <FondosTotalComponent></FondosTotalComponent>
        <FondosChartComponent></FondosChartComponent>
    </div>
</template>

<style>
    h1 {
        color:green;
    }
</style>

<script>
// @ is an alias to /src
import FondosGridComponent from '@/components/fondos/FondosGridComponent.vue'
import FondosTotalComponent from '@/components/fondos/FondosTotalComponent.vue'
import FondosChartComponent from '@/components/fondos/FondosChartComponent.vue'

export default {
  components: {
    FondosGridComponent,
    FondosTotalComponent,
    FondosChartComponent
  }
}
</script>
