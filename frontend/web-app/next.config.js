/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: {
    serverActions: true,
  },
  images: {
    domains: ['cdn.pixabay.com'],
  },
}

export default nextConfig
