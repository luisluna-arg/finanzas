<template>
  <div class="home">
    <DashboardComponent msg="Welcome to Your Vue.js App"/>
  </div>
</template>

<script>
// @ is an alias to /src
import DashboardComponent from '@/components/DashboardComponent.vue'

export default {
  name: 'DashboardView',
  components: {
    DashboardComponent
  }
}
</script>
