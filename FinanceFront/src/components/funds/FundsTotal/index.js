import React, { useEffect, useState } from 'react';
import "./styles.scss";
import { dateHelpers } from '../../../utils/commons';
import movementsApi from '../../../api/movementsApi';
import moment from 'moment';

const componentBaseClass = `card border-info mb-3 mt-3 text-dark FundsTotal`;

const defaultTotals = {
  timeStamp: moment(new Date()).format(),
  income: 0.0,
  funds: 0.0,
  lastDeposit: 0.0
};

const FundsTotal = () => {
  const [totals, setTotals] = useState(defaultTotals)
  const [hasError, setHasError] = useState(false)
  const [error, setError] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let isCancelled = false

    if (isCancelled === false) setIsLoading(true)

    movementsApi.totals(
      (response) => {
        setIsLoading(false);
        if (response.data.success) {
          setError(null);
          setHasError(false);
          setTotals(response.data);
        }
        else {
          setError(response.data.message);
          setHasError(true);
          setTotals(defaultTotals);
        }
      },
      (reqError) => {
        setIsLoading(false);
        setHasError(true);
        let errorMessage = `${reqError.code} - ${reqError.message}`;
        setError(errorMessage);
        console.error(errorMessage);
      },
    )

    // clean up render
    return () => { isCancelled = true }
  }, [setIsLoading, setError, setHasError]);

  if (isLoading) return (
    <div className={componentBaseClass}>
      <div className={`pt-3 ps-3 pe-3 pb-2`}>
        <h6>Cargando...</h6>
      </div>
    </div>
  )

  if (hasError) {
    return (
      <div className={componentBaseClass}>
        <div className={`ps-3 pe-3 pb-2 pt-2`}>
          {error}
        </div>
      </div>
    );
  }
  else {
    return (
      <div className={`${componentBaseClass}`}>
        <div className="card border-secondary">
          <div className="card-header">Header</div>
          <div className="card-body text-secondary p-1 p-3">
            <table className="table table-hover table-secondary mb-0">
              <thead>
                <tr>
                  <th scope="col">Fecha</th>
                  <th scope="col">Ingresos</th>
                  <th scope="col">Fondos</th>
                  <th scope="col">Fondos Ult. Depósito</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <th scope="row">{dateHelpers.format(totals.timeStamp)}</th>
                  <td>{totals.income}</td>
                  <td>{totals.funds}</td>
                  <td>{totals.lastDeposit}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    );
  }
};

// FundsTotal.propTypes = {};

export default FundsTotal;
