import NextAuth, { NextAuthOptions } from 'next-auth'
import DuendeProvider from 'next-auth/providers/duende-identity-server6'

export const authOptions: NextAuthOptions = {
  session: {
    strategy: 'jwt',
  },
  providers: [
    DuendeProvider({
      id: 'id-server', // Unique identifier for the provider
      clientId: 'nextApp', // Client ID as defined in the Identity service config
      clientSecret: 'secret', // Client secret as defined in the Identity service config
      issuer: 'http://localhost:5000', // The issuer URL of the Identity server
      authorization: { params: { scope: 'openid profile auctionApp' } }, // Authorization parameters including scopes
      idToken: true, // Include ID Token in the response
    }),
  ],
  callbacks: {
    async jwt({ token, profile, account }) {
      if (profile) {
        token.username = profile.username
      }
      if (account) {
        token.access_token = account.access_token
      }
      return token
    },
    async session({ session, token }) {
      if (token) {
        session.user.username = token.username
      }
      return session
    },
  },
}

const handler = NextAuth(authOptions) // Initialize NextAuth with the defined options
export { handler as GET, handler as POST } // Export the handler for GET and POST requests
