<template>
  <PieChart />
</template>

<style>
/**
 * The default size is 600px×400px, for responsive charts
 * you may need to set percentage values as follows (also
 * don't forget to provide a size for the container).
 */
.echarts {
  width: 100%;
  height: 100%;
}
</style>

<script>
import PieChart from '../utils/PieChart';

export default {
  components: {
    PieChart,
  },
};
</script>