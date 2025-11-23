/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: ["class"],
    content: [
        './pages/**/*.{ts,tsx}',
        './components/**/*.{ts,tsx}',
        './app/**/*.{ts,tsx}',
        './src/**/*.{ts,tsx}',
    ],
    prefix: "",
    theme: {
        container: {
            center: true,
            padding: "2rem",
            screens: {
                "2xl": "1400px",
            },
        },
        extend: {
            fontFamily: {
                display: ["Cormorant Garamond", "serif"],
                body: ["Inter", "sans-serif"],
            },
            colors: {
                border: "hsl(var(--border))",
                background: "hsl(var(--background))",
                primary: {
                    DEFAULT: "hsl(var(--primary))",
                },
                accent: {
                    DEFAULT: "hsl(var(--accent))",
                },
            },
            borderRadius: {
                lg: "0.5rem",
                md: "calc(0.5rem - 2px)",
                sm: "calc(0.5rem - 4px)",
            },
            transitionTimingFunction: {
                'smooth': 'cubic-bezier(0.4, 0, 0.2, 1)',
            },
            backgroundImage: {
                'hero-gradient': 'radial-gradient(circle, rgba(235,232,228,1) 0%, rgba(244,241,238,1) 100%)',
            }
        },
    },
    plugins: [require("tailwindcss-animate")],
}