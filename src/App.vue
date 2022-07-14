<template>
  <div id="app">
    <Home v-if="settings == true"/>
  </div>
</template>

<script>
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

import Home from './components/Home.vue'

export default {
  name: 'App',
  components: {
    Home
},
  data() {
      return {
          settings: false
      }
  },
  mounted() {
    console.log("I am in mounted!!!")
    this.settings = true
  },
  setup () {
    console.log("I am in setup!!!")
  }
}
</script>

<style>
  @import './assets/styles/global.css';
</style>