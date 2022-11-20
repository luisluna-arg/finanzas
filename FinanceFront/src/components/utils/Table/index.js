import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';

/* https://reactjs.org/docs/lists-and-keys.html 
*  https://mui.com/material-ui/react-table/
*/

function TableCustomRow(columns) {
    const column = columns;
    const listItems = column.map((col) =>
        <TableCell align="right">{col.value}</TableCell>
    );
    return (<TableRow>{listItems}</TableRow>);
}

export default function BasicTable() {
    let headerRow = props.headerRow;
    let tableRows = props.rows;

    /* <TableContainer component={Paper}> */
    return (
        <TableContainer>
            <Table>
                <TableHead>
                    <TableCustomRow columns={headerRow}></TableCustomRow>
                </TableHead>
                <TableBody>
                    {tableRows.map((row) => (
                        <TableCustomRow columns={row}></TableCustomRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}