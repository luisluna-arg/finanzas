import styles from './styles.module.css';
import React, { useEffect } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import useMovementsStore from "../../../zustand/stores/generic";
import { shallow } from 'zustand/shallow';
import { ApiUrls, APIs } from '../../../utils/commons';

const columns = [
    { field: 'id', headerName: 'ID', width: 70, hide: true },
    { field: 'timeStamp', headerName: 'Fecha', width: 200 },
    { field: 'concept1', headerName: 'Concepto 1', width: 400 },
    { field: 'concept2', headerName: 'Concepto 2', width: 400 },
    { field: 'ammount', headerName: 'Movimiento', width: 160 },
    { field: 'total', headerName: 'Total', width: 160 },
];

const FundsGrid = () => {

    const { getAll, results, isLoading /*, hasError, errorMessage*/ } = useMovementsStore(state => ({
        getAll: state.getAll,
        results: state.results,
        isLoading: state.isLoading/*,
        hasError: state.hasError,
        errorMessage: state.errorMessage*/
    }), shallow); // Using zustand


    useEffect(() => {
        getAll(ApiUrls[APIs.Movement].all).catch((a) => {
            console.log("a", a);
        });
        //eslint-disable-next-line
    }, [])

    if (isLoading) return (<div>Cargando...</div>)

    console.log("results", results);

    return (
        <div class="table table-striped table-dark text-light h-100">
            <DataGrid
                rows={results ?? []}
                columns={columns}
                pageSize={12}
                rowsPerPageOptions={[5]}
                checkboxSelection
            />
        </div>
    )
};

FundsGrid.propTypes = {};

export default FundsGrid;
