/** @type {import('next').NextConfig} */
const nextConfig = {};

module.exports = {
  ...nextConfig,
  //fix error: No 'Access-Control-Allow-Origin' header is present on the requested resource
  async rewrites() {
    return [
      {
        source: "*",
        destination: "*",
      },
    ];
  },
};
