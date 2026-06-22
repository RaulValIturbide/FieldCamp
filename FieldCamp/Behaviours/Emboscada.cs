using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    public class Emboscada
    {
        private const float BASE = 0.40f;
        private const float PESO_TACTICA = 0.002f;
        private const float PESO_SUBTERFUGIO = 0.002f;
        private const float TECHO = 0.90f;
        private const float HORAS_IGNORE = 3f;   // colchón > intervalo de renovación
        private const int MAX_INTENTO = 4;       // horas de cooldown tras un fallo

        private static int contadorSiguienteIntento = 0;

        public static void OnHourlyTick()
        {
            // 1) El cooldown de reintento corre SIEMPRE,
            //    estés oculto o no (si no, nunca se recupera tras un fallo).
            if (contadorSiguienteIntento > 0)
                contadorSiguienteIntento--;

            // 2) El mantenimiento del "ignórame" solo si estás realmente oculto.
            if (QuestManager._IsCamping && QuestManager._IsHiding)
                AplicarOcultacion();   // empuja el "ignórame" otras 3h hacia delante
        }

        public static bool IntentarOcultarse()
        {
            // ¿Aún en cooldown desde el último fallo? No dejamos reintentar.
            if (contadorSiguienteIntento > 0)
            {
                MBInformationManager.AddQuickInformation(
                    new TextObject("{=ambush_out_of_try_timer}You need to let some time pass since your last attempt."), 0);
                return false;
            }

            float prob = ProbabilidadOcultacion();
            bool exito = MBRandom.RandomFloat <= prob;

            if (exito)
            {
                QuestManager._IsHiding = true;
                AplicarOcultacion();   // ignore + icono

                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=ambush_hide_success}Your party melts into the surroundings, unseen.").ToString()
                    , new Color(0.4f, 0.8f, 0.4f)));
                MBInformationManager.AddQuickInformation(
                    new TextObject("{=ambush_hide_success_popup}The ambush was set up successfully."), 0,
                    soundEventPath: "event:/ui/notification/levelup");
            }
            else
            {
                QuestManager._IsHiding = false;
                contadorSiguienteIntento = MAX_INTENTO;   // arranca el cooldown

                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=ambush_hide_fail}Your men fail to conceal themselves.").ToString()
                    , new Color(1f, 0.6f, 0f)));
                MBInformationManager.AddQuickInformation(
                    new TextObject("{=ambush_hide_fail_popup}Failed to set up the ambush."), 0,
                    soundEventPath: "event:/ui/notification/relation");
            }
            return exito;
        }

        // Aplica/renueva el efecto. Se llama al activar Y en cada tick mientras _IsHiding.
        public static void AplicarOcultacion()
        {
            MobileParty p = MobileParty.MainParty;
            if (p == null) return;

            p.IgnoreByOtherPartiesTill(CampaignTime.HoursFromNow(HORAS_IGNORE));
            p.IsVisible = false;                 // capa cosmética
            p.Party.SetVisualAsDirty();
        }

        private static float ProbabilidadOcultacion()
        {
            int tactica = Hero.MainHero.GetSkillValue(DefaultSkills.Tactics);
            int subterfugio = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);

            float prob = BASE + (tactica * PESO_TACTICA) + (subterfugio * PESO_SUBTERFUGIO);
            if (prob < 0.1f)
                prob = 0.1f;
            return Math.Min(prob, TECHO);
        }

        public static void DesactivarOcultacion()
        {
            MobileParty p = MobileParty.MainParty;
            if (p != null)
            {
                p.IsVisible = true;
                // corta el ignore YA, sin esperar a que caduque
                p.IgnoreByOtherPartiesTill(CampaignTime.Now);
                p.Party.SetVisualAsDirty();
            }
            QuestManager._IsHiding = false;
            contadorSiguienteIntento = 0;   // limpio para el próximo campamento
        }
    }
}