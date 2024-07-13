export { default } from 'next-auth/middleware'

// protected routes
export const config = {
  matcher: ['/session'],
  pages: {
    signIn: '/api/auth/signin',
  },
}
