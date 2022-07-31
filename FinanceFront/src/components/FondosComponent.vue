<template>
  <div>
    <h1>Fondos</h1>
    <ImportButtonComponent :UploadType="FundsUploadType"></ImportButtonComponent>
    <!-- <FondosChartComponent></FondosChartComponent> -->
    <FondosGridComponent></FondosGridComponent>
    <FondosTotalComponent></FondosTotalComponent>
    <CRUDPopUp :editorSettings="CRUDPopUpSettings" formId="fundsForm" />
  </div>
</template>

<style>
h1 {
  color: green;
}
</style>

<script>
// @ is an alias to /src
import FondosGridComponent from '@/components/fondos/FondosGridComponent.vue'
import FondosTotalComponent from '@/components/fondos/FondosTotalComponent.vue'
// import FondosChartComponent from '@/components/fondos/FondosChartComponent.vue'
import ImportButtonComponent from '@/components/utils/ImportButtonComponent.vue'
import { UploadTypes } from '@/utils/commons'
import CRUDPopUp from '@/components/utils/CRUDPopUp.vue'

export default {
  components: {
    FondosGridComponent,
    FondosTotalComponent,
    // FondosChartComponent,
    ImportButtonComponent,
    CRUDPopUp
  },
  data() {
    return {
      FundsUploadType: UploadTypes.Funds,
      CRUDPopUpSettings: [
        { id: "name", type: "TextInput", label: "Nombre" },
        { id: "lastname", type: "TextInput", label: "Apellido" }
      ]
    }
  }
}
</script>
