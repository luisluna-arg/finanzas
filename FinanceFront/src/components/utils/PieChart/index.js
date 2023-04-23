import React from 'react';
// import PropTypes from 'prop-types';
// import styles from './styles.css';
// import the core library.
import ReactEChartsCore from 'echarts-for-react/lib/core';
// Import the echarts core module, which provides the necessary interfaces for using echarts.
import * as echarts from 'echarts/core';
// Import charts, all with Chart suffix
import { PieChart } from 'echarts/charts';
// import components, all suffixed with Component
import { GridComponent, TooltipComponent, TitleComponent, DatasetComponent } from 'echarts/components';
// Import renderer, note that introducing the CanvasRenderer or SVGRenderer is a required step
import { CanvasRenderer } from 'echarts/renderers';

const PieChart = () => {
  // Register the required components
  echarts.use(
    [TitleComponent, TooltipComponent, GridComponent, BarChart, CanvasRenderer]
  );
  return (
    // The usage of ReactEChartsCore are same with above.
    <ReactEChartsCore
      echarts={echarts}
      option={this.getOption()}
      notMerge={true}
      lazyUpdate={true}
      theme={"theme_name"}
      onChartReady={this.onChartReadyCallback}
      onEvents={EventsDict}
      // opts=null
    />
  );
};

PieChart.propTypes = {};

PieChart.defaultProps = {};

export default PieChart;
