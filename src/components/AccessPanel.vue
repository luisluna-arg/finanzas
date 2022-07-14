<template>
  <div class="panel">
        <label>{{PanelName}}</label>
        <button>Importar</button>
  </div>
</template>

<script>
export default {
  name: 'AccessPanel',
  props: ['PanelName'],
  components: {
  },
  data() {
      return {
          
      }
  },
  methods: {
    
  },
  mounted () {
    this.PanelName = "Fondos";
  }
}
</script>