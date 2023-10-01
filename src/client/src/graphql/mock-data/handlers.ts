import { graphql } from 'msw';

import * as MockBankingInstitutionsData from './GetBankingInstitutions.json';

export const handlers = [
  graphql.query('GetBankingInstitutions', (req, res, ctx) => {
    const { countryCode } = req.variables;
    console.log(countryCode);

    return res(ctx.data(MockBankingInstitutionsData.data));
  }),
];
