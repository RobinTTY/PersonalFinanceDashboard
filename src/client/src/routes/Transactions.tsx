import { AgGridReact } from 'ag-grid-react';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import { useCallback, useMemo, useRef } from 'react';
import { CellClickedEvent, GridReadyEvent } from 'ag-grid-community';

export const Transactions = () => {
  const gridRef = useRef<AgGridReact>(null);
  const defaultColDef = useMemo(
    () => ({
      sortable: true,
      filter: true,
    }),
    []
  );

  const cellClickedHandler = useCallback((event: CellClickedEvent) => {
    console.log(event);
  }, []);

  const onGridReady = useCallback((params: GridReadyEvent) => {
    params.api.sizeColumnsToFit();
  }, []);

  const rowData: any[] = [
    { date: '1/1/2020', payee: 'Target', category: 'Shopping', payment: 100, deposit: 0 },
    { date: '1/2/2020', payee: 'Best Buy', category: 'Shopping', payment: 200, deposit: 0 },
    { date: '1/3/2020', payee: 'Paycheck', category: 'Income', payment: 0, deposit: 1000 },
    { date: '1/4/2020', payee: 'Shell', category: 'Gas', payment: 50, deposit: 0 },
    { date: '1/5/2020', payee: 'Target', category: 'Shopping', payment: 100, deposit: 0 },
    { date: '1/6/2020', payee: 'Best Buy', category: 'Shopping', payment: 200, deposit: 0 },
  ];

  const columnDefs: any[] = [
    { headerName: 'Date', field: 'date' },
    { headerName: 'Payee', field: 'payee' },
    { headerName: 'Category', field: 'category' },
    { headerName: 'Payment', field: 'payment' },
    { headerName: 'Deposit', field: 'deposit' },
  ];

  return (
    <div className="ag-theme-alpine-dark" style={{ height: '100%', width: '100%' }}>
      <AgGridReact
        ref={gridRef}
        rowData={rowData}
        columnDefs={columnDefs}
        defaultColDef={defaultColDef}
        rowSelection="multiple"
        animateRows
        onCellClicked={cellClickedHandler}
        onGridReady={onGridReady}
        onGridSizeChanged={() => gridRef.current?.api?.sizeColumnsToFit()}
      />
    </div>
  );
};
