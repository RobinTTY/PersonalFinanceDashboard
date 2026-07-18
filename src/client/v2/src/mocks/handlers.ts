import { graphql, HttpResponse } from 'msw';
import { dashboardData } from './data/dashboard';

/**
 * MSW request handlers for the GraphQL API.
 *
 * Handlers are matched by GraphQL operation name, independent of the endpoint URL, so
 * they intercept the requests Apollo Client sends regardless of which `uri` it targets.
 * Add a new `graphql.query`/`graphql.mutation` handler here for each operation you want
 * to serve from a mock instead of the real backend.
 */
export const handlers = [
  graphql.query('GetNetWorthHistory', () => HttpResponse.json({ data: dashboardData })),
];
