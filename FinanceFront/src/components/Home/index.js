import React from 'react';
import PropTypes from 'prop-types';
import styles from './styles.scss';

const Home = () => (
    <div className={styles.Home}>
        <h1>Este es el Home</h1>
    </div>
);

Home.propTypes = {
    msg: PropTypes.string
};

Home.defaultProps = {
    msg: 'Este es el Home'
};

export default Home;
