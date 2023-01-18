import { EnvironmentPlugin } from 'webpack';
import { config } from 'dotenv';

config();

module.exports = {
  plugins: [
    new EnvironmentPlugin([
      'GOOGLE_CLIENT_ID'
    ]),
  ],
};
