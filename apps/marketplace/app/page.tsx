import { Button, ThemeToggler } from '@ecommerce/ui';

export default function Home() {
  return (
    <>
      <div className="mt-10 bg-primary text-3xl font-bold">Marketplace app</div>
      <div className="bg-red-500">TEST</div>
      <div className="flex flex-col items-center justify-center">
        <ThemeToggler />
        <Button variant="default">Shadcn Button from ui package - default</Button>
        <Button variant="destructive">Shadcn Button from ui package - destructive</Button>
        <Button variant="outline">Shadcn Button from ui package - outline</Button>
        <Button variant="secondary">Shadcn Button from ui package - secondary</Button>
        <Button variant="ghost">Shadcn Button from ui package - ghost</Button>
        <Button variant="link">Shadcn Button from ui package - link</Button>

        <Button size="xs">button-xs</Button>
        <Button size="sm">button-sm</Button>
        <Button size="sm">button-lg</Button>
        <Button size="icon">icon</Button>
        <Button size="icon-xs">xs</Button>
        <Button size="icon-sm">sm</Button>
        <Button size="icon-lg">lg</Button>
      </div>
    </>
  );
}
