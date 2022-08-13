import React from 'react';
import PropTypes from 'prop-types';
import styles from './styles.css';

const Dashboard = () => (
  <div className={styles.Dashboard}>
    <h1>Este es el Dashboard</h1>
  </div>
);

Dashboard.propTypes = {
  msg: PropTypes.string
};

Dashboard.defaultProps = {
  msg: 'Este es el dashboard'
};

export default Dashboard;
