export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex h-full flex-col overflow-hidden">
      <header className="flex shrink-0 items-center justify-between">
        <div>Company logo</div>
        <nav aria-label="Primary Navigation">
          <ul className="flex gap-x-4">
            <li>Navigation 1</li>
            <li>Navigation 2</li>
          </ul>
        </nav>
      </header>
      <main className="flex-1 overflow-auto">{children}</main>
      <footer className="shrink-0">footer here</footer>
    </div>
  );
}
