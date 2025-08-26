using Rogue.Domain.Characters;
using Rogue.Domain.Items;

namespace Rogue.Domain;

public interface INotifier
{
    void PlayerAttacked(Monster monster);
    void PlayerMissed(Monster monster);
    void MonsterAttacked(Monster monster);
    void MonsterMissed(Monster monster);
    void PlayerPickedUp(Item item);
    void FullBackpack(Item item);
}
