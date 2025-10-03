# Job Application Tracker - Frontend

A modern Next.js frontend for tracking job applications, interviews, and offers.

## Tech Stack

- **Framework:** Next.js 15 with App Router
- **Language:** TypeScript
- **Styling:** Tailwind CSS 4
- **UI Components:** shadcn/ui
- **State Management:** React Query (TanStack Query)
- **HTTP Client:** Axios
- **Form Handling:** React Hook Form + Zod
- **Charts:** Recharts
- **Icons:** Lucide React
- **Package Manager:** pnpm

## Getting Started

### Prerequisites

- Node.js 18+
- pnpm 9+

### Installation

1. Install dependencies:
```bash
pnpm install
```

2. Create environment file:
```bash
cp .env.local.example .env.local
```

3. Update `.env.local` with your API URL:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

### Development

Run the development server:
```bash
pnpm dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### Build

Build for production:
```bash
pnpm build
```

Start production server:
```bash
pnpm start
```

## Project Structure

```
src/
├── app/
│   ├── (dashboard)/          # Dashboard layout group
│   │   ├── page.tsx          # Dashboard home
│   │   ├── applications/     # Applications pages
│   │   ├── calendar/         # Calendar page
│   │   └── analytics/        # Analytics page
│   ├── layout.tsx            # Root layout
│   └── globals.css           # Global styles
├── components/
│   ├── ui/                   # shadcn/ui components
│   ├── navigation.tsx        # Main navigation
│   └── theme-toggle.tsx      # Dark mode toggle
└── lib/
    ├── api.ts                # Axios instance
    ├── types.ts              # TypeScript types
    ├── providers.tsx         # React Query & Theme providers
    └── utils.ts              # Utility functions
```

## Features

### Current Features
- ✅ Dashboard with statistics cards
- ✅ Applications list with filtering
- ✅ Application detail view with tabs
- ✅ Responsive navigation
- ✅ Dark mode support
- ✅ Type-safe API client
- ✅ Modern UI with shadcn/ui

### Upcoming Features
- 🚧 Calendar view with interview scheduling
- 🚧 Analytics charts and visualizations
- 🚧 Add/Edit application forms
- 🚧 Interview management
- 🚧 Notes and contacts
- 🚧 Real API integration
- 🚧 Search and advanced filtering
- 🚧 Export functionality

## Available Scripts

- `pnpm dev` - Start development server
- `pnpm build` - Build for production
- `pnpm start` - Start production server
- `pnpm lint` - Run ESLint

## Adding shadcn/ui Components

To add new components from shadcn/ui:

```bash
pnpm dlx shadcn@latest add [component-name]
```

Example:
```bash
pnpm dlx shadcn@latest add form
```

## Contributing

This project is part of Hacktoberfest 2025. See the main project README for contribution guidelines.

## License

See the main project LICENSE file.
