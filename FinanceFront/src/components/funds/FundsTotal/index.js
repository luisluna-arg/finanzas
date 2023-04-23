import React, { useEffect, useState } from 'react';
import styles from './styles.scss';
import movementsApi from './../../../api/movementsApi';

const FundsTotal = (props) => {

  const [totals, setTotals] = useState([])
  const [hasError, setHasError] = useState(false)
  const [error, setError] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let isCancelled = false
    if (isCancelled === false)
      setIsLoading(true)
      movementsApi.totals(
        (result) => setTotals(result.data),
        (reqError) => {
          setHasError(true);
          setError(reqError);
        },
        () => setIsLoading(false)
        );

    // clean up render
    return () => { isCancelled = true }
  }, []);

  if (isLoading) return (<div>Cargando...</div>)

  if (!hasError) {
    return (
      <div className={styles.FundsTotal}>
        FundsTotal Component
      </div>
    );
  }
  else {
    let errorMessage = "Request failed";
    return (
      <div>
        {errorMessage}
      </div>
    );
  }

};

// FundsTotal.propTypes = {};

export default FundsTotal;
