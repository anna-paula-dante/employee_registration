import type { Config } from 'tailwindcss'

export default {
  content: ['./index.html','./src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        brand: {
          50:  '#f8fafc',
          100: '#f1f5f9',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b',
          900: '#0f172a'
        },
        accent: {
          50:  '#eef2ff',
          100: '#e0e7ff',
          200: '#c7d2fe',
          300: '#a5b4fc',
          400: '#818cf8',
          500: '#6366f1',
          600: '#4f46e5',
          700: '#4338ca',
          800: '#3730a3',
          900: '#312e81'
        },
        success: {
          500: '#10b981',
          600: '#059669'
        }
      },
      boxShadow: {
        card: '0 10px 30px -12px rgba(2,6,23,0.18), 0 6px 14px -8px rgba(2,6,23,0.12)'
      }
    }
  },
  plugins: []
} satisfies Config
