<template>
    <div>
        <h1>Fondos</h1>    
        <ImportButtonComponent :UploadType="FundsUploadType"></ImportButtonComponent>
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
import ImportButtonComponent from '@/components/utils/ImportButtonComponent.vue'
import { UploadTypes } from '@/utils/commons'

export default {
  components: {
    FondosGridComponent,
    FondosTotalComponent,
    FondosChartComponent,
    ImportButtonComponent
  },
  data() {
    return { FundsUploadType: UploadTypes.Funds }
  }
}
</script>
