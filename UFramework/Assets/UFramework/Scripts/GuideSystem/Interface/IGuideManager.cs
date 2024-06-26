﻿namespace UFramework
{
    public interface IGuideManager
    {
        void Init();

        void LocalUpdate(float dt);

        bool IsInGuiding();

        bool IsInGuide(GuideID guideID);
    }
}