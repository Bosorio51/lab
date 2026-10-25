import { useState } from "react";
import { ItemPicker } from "@/components/mobile/ItemPicker";
import { NewItemDialog } from "@/components/mobile/NewItemDialog";
import type { PickItem } from "@/hooks/UsePickItems";

type ReceiveItemChooserProps = {
  onSelect: (item: PickItem) => void;
};

// The first step of Receive: goods arrive, so an item can be added if it is not already listed.
export function ReceiveItemChooser({ onSelect }: ReceiveItemChooserProps) {
  const [newItemName, setNewItemName] = useState<string | undefined>();
  const [newItemOpen, setNewItemOpen] = useState(false);

  return (
    <div>
      <ItemPicker
        onSelect={onSelect}
        onCreate={(search) => {
          setNewItemName(search || undefined);
          setNewItemOpen(true);
        }}
      />

      <NewItemDialog
        open={newItemOpen}
        onOpenChange={setNewItemOpen}
        defaults={{ name: newItemName }}
        onCreated={(item) => {
          setNewItemOpen(false);
          onSelect(item);
        }}
      />
    </div>
  );
}
