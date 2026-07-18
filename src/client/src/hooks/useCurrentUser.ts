export interface CurrentUser {
  name: string;
  email?: string;
  avatarUrl?: string;
}

/**
 * Returns the currently authenticated user.
 *
 * The backend does not expose a user query yet, so this currently returns static placeholder data.
 * Once a `currentUser`/`me` query exists, swap the body for a `useQuery` call — Apollo's normalized
 * cache then acts as the global store, so every consumer reads the same user from a single fetch
 * with no prop-drilling and no bespoke context provider.
 */
export function useCurrentUser(): CurrentUser {
  return {
    name: 'Robin Müller',
    email: 'muellerobin95@gmail.com',
    avatarUrl:
      'https://avatars.githubusercontent.com/u/27669180',
  };
}
