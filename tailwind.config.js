/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: "class",
  content: [
    "./Views/**/*.cshtml",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#2F5FF0",
          hover: "#2748C4",
        },
        secondary: {
          DEFAULT: "#6c757d",
          hover: "#5c636a",
        },
        success: {
          DEFAULT: "#639922",
          hover: "#4f7a1b",
        },
        danger: {
          DEFAULT: "#DC2626",
          hover: "#B91C1C",
        },
        warning: {
          DEFAULT: "#D97706",
          hover: "#B45F04",
        },
        info: {
          DEFAULT: "#0891B2",
          hover: "#06748F",
        },
      },
      fontFamily: {
        sans: ["Inter", "system-ui", "sans-serif"],
      },
    },
  },
  plugins: [],
}

