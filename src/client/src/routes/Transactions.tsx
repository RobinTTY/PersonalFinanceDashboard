import { AgGridReact } from 'ag-grid-react';
import { useCallback, useMemo, useRef } from 'react';
import {
  CellClickedEvent,
  GridReadyEvent,
  ICellRendererParams,
  ValueFormatterFunc,
  ValueFormatterParams,
} from 'ag-grid-community';
import { useQuery } from "@apollo/client/react";
import { Badge } from '@mantine/core';
import { GetTransactionsQuery } from '@/graphql/queries/GetTransactions';
import { getCurrencyFormatter } from '@/utility/getCurrencyFormatter';

import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';
import './GlobalGridStyles.css';
import { getDateFormatter } from '@/utility/getDateFormatter';

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

  // TODO: locale should be set by user preference
  const locale = 'en-US';
  const dateFormatter = getDateFormatter(locale, 'medium');
  const currencyFormatter = getCurrencyFormatter(locale, 'USD');
  const rowData = data?.transactions?.edges!.map((edge) => {
    const node = edge?.node;
    return {
      date: node.valueDate,
      payee: node.payee,
      category: node.category,
      activity: node.amount,
    };
  });

  const MyRenderer = (params: ICellRendererParams<number, number>) => {
    const activity = params.value ?? 0;
    const debit = activity < 0;
    return (
      <Badge size="lg" variant="light" color={debit ? 'red' : 'green'}>
        {currencyFormatter.format(activity)}
      </Badge>
    );
  };

  const agDateFormatter: ValueFormatterFunc<Date> = (params: ValueFormatterParams): string =>
    dateFormatter.format(params.value);

  const columnDefs: any[] = [
    { headerName: 'Date', field: 'date', valueFormatter: agDateFormatter },
    { headerName: 'Payee', field: 'payee' },
    { headerName: 'Category', field: 'category' },
    {
      headerName: 'Activity',
      field: 'activity',
      cellRenderer: MyRenderer,
    },
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
        suppressHorizontalScroll
        onCellClicked={cellClickedHandler}
        onGridReady={onGridReady}
        onGridSizeChanged={() => gridRef.current?.api?.sizeColumnsToFit()}
      />
    </div>
  );
};
