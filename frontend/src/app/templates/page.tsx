export default function TemplatesPage() {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight">Application Templates</h2>
        <p className="text-neutral-500">Save and reuse common application details.</p>
      </div>
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {['General Software Engineer', 'Frontend Developer', 'Backend .NET'].map((t) => (
          <div key={t} className="border rounded-xl p-6">
            <div className="font-semibold mb-1">{t}</div>
            <p className="text-sm text-neutral-600">Preset fields for quick apply.</p>
          </div>
        ))}
      </div>
    </div>
  );
}


