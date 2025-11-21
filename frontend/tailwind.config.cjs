/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'ui-sans-serif', 'system-ui']
      },
      colors: {
        accent: {
          DEFAULT: '#4f46e5',
          dark: '#4338ca'
        }
      }
    }
  },
  plugins: []
};
