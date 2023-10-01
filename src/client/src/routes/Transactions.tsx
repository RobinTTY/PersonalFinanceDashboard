import { AgGridReact } from 'ag-grid-react';
import { useCallback, useMemo, useRef } from 'react';
import { CellClickedEvent, GridReadyEvent } from 'ag-grid-community';
import { useQuery } from '@apollo/client';
import { GetTransactionsQuery } from '@/graphql/queries/GetTransactions';

import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import './GlobalGridStyles.css';

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

  const { data } = useQuery(GetTransactionsQuery, {
    variables: {
      accountId: '072fefa4-4530-4322-aafe-e953d37402ae',
      first: 15,
    },
  });

  const rowData = data?.transactions?.edges!.map((edge) => {
    const node = edge?.node;
    return {
      date: node.valueDate,
      payee: node.payee,
      category: node.category,
      payment: node.amount,
      deposit: 0,
    };
  });

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
