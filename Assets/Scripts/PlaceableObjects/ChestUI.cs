
public class ChestUI : InventoryUI{
    int NUMBER_OF_COLUMN;
        public override void OnNetworkSpawn()
    {
        if (!IsOwner){
            gameObject.SetActive(false);
        }
    }
}