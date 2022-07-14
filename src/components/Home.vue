<template>
  <div class="home">
    <AccessPanel :numberOfUsers="Fondos"></AccessPanel>
  </div>
</template>

<script>
import AccessPanel from './AccessPanel.vue';

export default {
  name: 'Home',
  components: {
    AccessPanel
  },
  data() {
      return {
          
      }
  },
  methods: {
    
  },
  mounted () {
    this.getAllUsers();
  }
}
</script>