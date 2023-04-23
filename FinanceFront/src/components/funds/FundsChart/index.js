import React from 'react';
// import PropTypes from 'prop-types';
import styles from './styles.scss';
import PieChart from '../../utils/PieChart';

const FundsChart = () => (
  <div className={styles.FundsChart}>
      <PieChart />
  </div>
);

FundsChart.propTypes = {};

export default FundsChart;
