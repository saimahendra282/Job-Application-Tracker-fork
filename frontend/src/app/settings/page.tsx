export default function SettingsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold tracking-tight">Settings</h2>
        <p className="text-neutral-500">Configure preferences for your job tracker.</p>
      </div>
      <div className="grid gap-4 md:grid-cols-2">
        <div className="border rounded-xl p-6 space-y-3">
          <div className="font-semibold">General</div>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" className="accent-blue-600" defaultChecked />
            Enable reminders
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" className="accent-blue-600" />
            Show salary fields by default
          </label>
        </div>
        <div className="border rounded-xl p-6 space-y-3">
          <div className="font-semibold">Notifications</div>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" className="accent-blue-600" defaultChecked />
            Weekly summary email
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" className="accent-blue-600" />
            Browser notifications
          </label>
        </div>
      </div>
    </div>
  );
}


