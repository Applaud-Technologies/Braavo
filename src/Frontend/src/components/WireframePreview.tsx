interface WireframePreviewProps {
  html: string;
  screenName: string;
}

export default function WireframePreview({ html, screenName }: WireframePreviewProps) {
  return (
    <div className="border border-gray-200 rounded-lg overflow-hidden bg-white">
      <div className="bg-gray-100 px-4 py-2 border-b border-gray-200">
        <h3 className="text-sm font-medium text-gray-700">{screenName}</h3>
      </div>
      <div
        className="p-4 min-h-[400px]"
        dangerouslySetInnerHTML={{ __html: html }}
      />
    </div>
  );
}
