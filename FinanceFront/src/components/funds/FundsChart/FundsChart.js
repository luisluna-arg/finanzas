import React from 'react';
import PropTypes from 'prop-types';
import styles from './FundsChart.module.css';
import PieChart from '../../utils/PieChart/PieChart';

const FundsChart = () => (
  <div className={styles.FundsChart}>
      <PieChart />
  </div>
);

FundsChart.propTypes = {};

FundsChart.defaultProps = {};

export default FundsChart;
