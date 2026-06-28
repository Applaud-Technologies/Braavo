/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Warm rustic primary palette
        primary: {
          50: '#fdf8f3',
          100: '#f9efe3',
          200: '#f3dfc7',
          300: '#e8c89e',
          400: '#d9a96e',
          500: '#c8894a',
          600: '#b8703c',
          700: '#995832',
          800: '#7d482e',
          900: '#663c28',
        },
        // Forest accent
        accent: {
          50: '#f4f9f4',
          100: '#e6f2e6',
          200: '#cce5cd',
          300: '#a3d0a5',
          400: '#72b376',
          500: '#4d9651',
          600: '#3b7a3f',
          700: '#316234',
          800: '#2a4f2d',
          900: '#244127',
        },
      },
      fontFamily: {
        sans: ['Plus Jakarta Sans', 'system-ui', 'sans-serif'],
        display: ['Crimson Pro', 'Georgia', 'serif'],
        mono: ['JetBrains Mono', 'monospace'],
      },
      borderRadius: {
        'xl': '0.875rem',
        '2xl': '1rem',
      },
      boxShadow: {
        'warm': '0 4px 14px 0 rgba(139, 90, 43, 0.08)',
        'warm-lg': '0 10px 25px -3px rgba(139, 90, 43, 0.1)',
      },
    },
  },
  plugins: [],
}
